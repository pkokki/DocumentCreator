import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MappingsDetailComponent } from './mappings-detail.component';

describe('MappingsDetailComponent', () => {
  let component: MappingsDetailComponent;
  let fixture: ComponentFixture<MappingsDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MappingsDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MappingsDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
