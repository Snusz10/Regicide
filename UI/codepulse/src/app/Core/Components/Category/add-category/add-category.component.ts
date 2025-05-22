import { Component, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms'
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { CategoryService } from '../../../../Services/category.service';
import { AddCategoryModel } from '../../../../Models/Category/add-category-request.model';

@Component({
  selector: 'app-add-category',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './add-category.component.html',
  styleUrl: './add-category.component.css'
})
export class AddCategoryComponent implements OnDestroy{

    model :AddCategoryModel
    private addCategorySubscription?: Subscription;

    constructor(private categoryService: CategoryService,
        private router: Router){
        this.model = {
            name: '',
            urlHandle: ''
        }
    }

  onCategorySubmit(){
    this.addCategorySubscription  = this.categoryService.addCategory(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/admin/categories')
      },
      error: () =>{
        console.log('Adding a category was a failure!');
      }
    });
  }

  ngOnDestroy(): void {
    this.addCategorySubscription?.unsubscribe();
  }
}
