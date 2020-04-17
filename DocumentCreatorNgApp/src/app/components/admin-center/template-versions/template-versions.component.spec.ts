import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateVersionsComponent } from './template-versions.component';

describe('TemplateVersionsComponent', () => {
  let component: TemplateVersionsComponent;
  let fixture: ComponentFixture<TemplateVersionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TemplateVersionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TemplateVersionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
