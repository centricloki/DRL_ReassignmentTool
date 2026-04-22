import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BrandStyleComponent } from './brand-style.component';

describe('BrandStyleComponent', () => {
  let component: BrandStyleComponent;
  let fixture: ComponentFixture<BrandStyleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BrandStyleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BrandStyleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
