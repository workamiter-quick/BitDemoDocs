import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchAttributeComponent } from './search-attribute.component';

describe('SearchAttributeComponent', () => {
  let component: SearchAttributeComponent;
  let fixture: ComponentFixture<SearchAttributeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SearchAttributeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchAttributeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
