import { TestBed } from '@angular/core/testing';

import { EmployeePairsService } from './employee-pairs.service';

describe('EmployeePairsService', () => {
  let service: EmployeePairsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmployeePairsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
