// src/app/helpers/response-parser.helper.ts

/**
 * Reusable helper to parse legacy @angular/http Response._body
 * Compatible with Angular 2-5 and TypeScript 2.4-2.9
 */
export class ResponseParser {

  public static parseLegacyResponse(response: any): any {
    try {
      if (!response || !response.ok) {
        return null;
      }

      if (response._body) {
        var stringified = JSON.stringify(response._body);
        var firstParse = JSON.parse(stringified);
        return JSON.parse(firstParse);
      }

      if (typeof response._body === 'string') {
        return JSON.parse(response._body);
      }

      return response._body;
    } catch (error) {
      console.error('ResponseParser: Failed to parse response', error);
      return null;
    }
  }

  public static getDataArray(response: any): Array<any> {
    var parsed = this.parseLegacyResponse(response);
    if (parsed && parsed.isSuccess && Array.isArray(parsed.data)) {
      return parsed.data;
    }
    return [];
  }

  public static isSuccess(response: any): boolean {
    var parsed = this.parseLegacyResponse(response);
    return parsed && parsed.isSuccess === true;
  }

  public static getErrorMessage(response: any, fallback: string): string {
    var parsed = this.parseLegacyResponse(response);
    if (parsed && parsed.message) {
      return parsed.message;
    }
    return fallback;
  }
}
