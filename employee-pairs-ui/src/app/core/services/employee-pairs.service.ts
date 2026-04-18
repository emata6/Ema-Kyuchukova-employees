import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeePairAnalysisResponse } from '../../models/employee-pair-analysis-response';
import { environment } from '../../../environment';

@Injectable({
  providedIn: 'root'
})
export class EmployeePairsService {
  private http = inject(HttpClient);

  private readonly apiUrl =
    `${environment.apiBaseUrl}${environment.endpoints.employeePairs}`;

  uploadCsv(file: File): Observable<EmployeePairAnalysisResponse> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<EmployeePairAnalysisResponse>(
      `${this.apiUrl}/upload`,
      formData
    );
  }
}