import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MappingsVersionTableComponent } from './mappings-version-table.component';

describe('MappingsVersionTableComponent', () => {
  let component: MappingsVersionTableComponent;
  let fixture: ComponentFixture<MappingsVersionTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MappingsVersionTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MappingsVersionTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
