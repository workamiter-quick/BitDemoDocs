import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LayerManangerComponent } from './layer-mananger.component';

describe('LayerManangerComponent', () => {
  let component: LayerManangerComponent;
  let fixture: ComponentFixture<LayerManangerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LayerManangerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LayerManangerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
