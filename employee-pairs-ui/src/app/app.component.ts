import { Component } from '@angular/core';
import { EmployeePairsComponent } from './features/employee-pairs/employee-pairs.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [EmployeePairsComponent],
  templateUrl: './app.component.html'
})
export class AppComponent {}