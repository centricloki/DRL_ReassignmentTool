import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserReassignmentComponent } from './user-reassignment.component';

describe('UserReassignmentComponent', () => {
  let component: UserReassignmentComponent;
  let fixture: ComponentFixture<UserReassignmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserReassignmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserReassignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
