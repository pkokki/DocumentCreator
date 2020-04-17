import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminCenterHomeComponent } from './admin-center-home.component';

describe('AdminCenterHomeComponent', () => {
  let component: AdminCenterHomeComponent;
  let fixture: ComponentFixture<AdminCenterHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdminCenterHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminCenterHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
