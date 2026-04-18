import { Component, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeePairsService } from '../../core/services/employee-pairs.service';
import { EmployeePairProjectResult } from '../../models/employee-pair-project-result';
import { EmployeePairAnalysisResponse } from '../../models/employee-pair-analysis-response';
import { CsvRowError } from '../../models/csv-row-error';

import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';

@Component({
  selector: 'app-employee-pairs',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatPaginatorModule,
    MatSortModule
  ],
  templateUrl: './employee-pairs.component.html'
})
export class EmployeePairsComponent {
  private service = inject(EmployeePairsService);

  selectedFile: File | null = null;
  result: EmployeePairAnalysisResponse | null = null;

  displayedColumns = ['employeeId1', 'employeeId2', 'projectId', 'daysWorked'];
  dataSource = new MatTableDataSource<EmployeePairProjectResult>([]);

  errorDisplayedColumns = ['rowNumber', 'rowContent', 'errorMessage'];
  errorDataSource = new MatTableDataSource<CsvRowError>([]);

  isLoading = false;
  errorMessage = '';

  private projectsPaginator!: MatPaginator;
  private errorsPaginator!: MatPaginator;
  private errorsSort!: MatSort;

  @ViewChild('projectsPaginator')
  set matProjectsPaginator(paginator: MatPaginator) {
    if (paginator) {
      this.projectsPaginator = paginator;
      this.dataSource.paginator = paginator;
    }
  }

  @ViewChild('errorsPaginator')
  set matErrorsPaginator(paginator: MatPaginator) {
    if (paginator) {
      this.errorsPaginator = paginator;
      this.errorDataSource.paginator = paginator;
    }
  }

  @ViewChild(MatSort)
  set matSort(sort: MatSort) {
    if (sort) {
      this.errorsSort = sort;
      this.errorDataSource.sort = sort;
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      this.errorMessage = '';
    }
  }

  onUpload(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a CSV file.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.service.uploadCsv(this.selectedFile).subscribe({
      next: (res) => {
        this.result = res;
        this.dataSource.data = res.commonProjects ?? [];
        this.errorDataSource.data = res.errors ?? [];
        this.isLoading = false;

        if (this.projectsPaginator) {
          this.dataSource.paginator = this.projectsPaginator;
        }

        if (this.errorsPaginator) {
          this.errorDataSource.paginator = this.errorsPaginator;
        }

        if (this.errorsSort) {
          this.errorDataSource.sort = this.errorsSort;
        }

        this.dataSource._updateChangeSubscription();
        this.errorDataSource._updateChangeSubscription();
      },
      error: (err) => {
        console.log('Upload error:', err);
        this.isLoading = false;
        this.result = null;
        this.dataSource.data = [];
        this.errorDataSource.data = [];
        this.errorMessage = err?.error?.message || err?.message || 'Error uploading file.';
      }
    });
  }
}