import { EmployeePairProjectResult } from './employee-pair-project-result';
import { CsvRowError } from './csv-row-error';

export interface EmployeePairAnalysisResponse {
  commonProjects: EmployeePairProjectResult[];
  topEmployeeId1: number | null;
  topEmployeeId2: number | null;
  totalDaysWorkedTogether: number;
  errors: CsvRowError[];
}