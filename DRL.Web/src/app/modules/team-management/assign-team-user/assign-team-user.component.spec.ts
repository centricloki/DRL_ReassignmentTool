import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignTeamUserComponent } from './assign-team-user.component';

describe('AssignTeamUserComponent', () => {
  let component: AssignTeamUserComponent;
  let fixture: ComponentFixture<AssignTeamUserComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssignTeamUserComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssignTeamUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
