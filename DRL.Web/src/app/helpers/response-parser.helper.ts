// src/app/helpers/response-parser.helper.ts
import { Response } from '@angular/http';

/**
 * Reusable helper to parse legacy @angular/http Response._body
 * Handles double-JSON-stringified responses from custom HttpService
 * 
 * Usage: 
 *   const parsed = ResponseParser.parseLegacyResponse(response);
 *   if (parsed?.isSuccess) { ... }
 */
export class ResponseParser {

  /**
   * Parse legacy Response object with double-encoded JSON body
   * @param response - Raw Response from @angular/http
   * @returns Parsed JSON object or null if parsing fails
   */
  public static parseLegacyResponse(response: any): any {
    try {
      if (!response || !response.ok) {
        return null;
      }

      // Handle Response._body (double-encoded JSON string)
      if (response._body) {
        const stringified = JSON.stringify(response._body);
        const firstParse = JSON.parse(stringified);
        return JSON.parse(firstParse);
      }

      // Fallback: try parsing body directly
      if (typeof response._body === 'string') {
        return JSON.parse(response._body);
      }

      return response._body;
    } catch (error) {
      console.error('ResponseParser: Failed to parse response', error);
      return null;
    }
  }

  /**
   * Safe getter for parsed response data array
   * @param response - Raw Response from @angular/http
   * @returns Array of data items or empty array
   */
  public static getDataArray(response: any): any[] {
    const parsed = this.parseLegacyResponse(response);
    return (parsed?.isSuccess && Array.isArray(parsed.data)) ? parsed.data : [];
  }

  /**
   * Check if response indicates success
   * @param response - Raw Response from @angular/http
   * @returns boolean success status
   */
  public static isSuccess(response: any): boolean {
    const parsed = this.parseLegacyResponse(response);
    return parsed?.isSuccess === true;
  }

  /**
   * Get error message from response
   * @param response - Raw Response from @angular/http
   * @param fallback - Default message if none found
   * @returns Error message string
   */
  public static getErrorMessage(response: any, fallback: string = 'An error occurred'): string {
    const parsed = this.parseLegacyResponse(response);
    return parsed?.message || fallback;
  }
}
