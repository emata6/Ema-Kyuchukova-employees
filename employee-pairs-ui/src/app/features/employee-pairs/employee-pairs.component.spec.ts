import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeePairsComponent } from './employee-pairs.component';

describe('EmployeePairsComponent', () => {
  let component: EmployeePairsComponent;
  let fixture: ComponentFixture<EmployeePairsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmployeePairsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmployeePairsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
