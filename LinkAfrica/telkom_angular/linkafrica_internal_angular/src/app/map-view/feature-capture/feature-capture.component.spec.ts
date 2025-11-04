import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FeatureCaptureComponent } from './feature-capture.component';

describe('FeatureCaptureComponent', () => {
  let component: FeatureCaptureComponent;
  let fixture: ComponentFixture<FeatureCaptureComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FeatureCaptureComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FeatureCaptureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
