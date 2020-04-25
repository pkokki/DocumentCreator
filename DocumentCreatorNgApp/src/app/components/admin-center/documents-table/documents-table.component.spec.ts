import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DocumentsTableComponent } from './documents-table.component';

describe('DocumentsTableComponent', () => {
  let component: DocumentsTableComponent;
  let fixture: ComponentFixture<DocumentsTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DocumentsTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DocumentsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
