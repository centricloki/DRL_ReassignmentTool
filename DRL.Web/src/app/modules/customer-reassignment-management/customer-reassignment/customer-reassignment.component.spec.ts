import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerReassignmentComponent } from './customer-reassignment.component';

describe('CustomerReassignmentComponent', () => {
  let component: CustomerReassignmentComponent;
  let fixture: ComponentFixture<CustomerReassignmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomerReassignmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerReassignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
