import { Routes } from '@angular/router';
import { CategoryListComponent } from './Core/Components/Category/category-list/category-list.component';
import { AddCategoryComponent } from './Core/Components/Category/add-category/add-category.component';
import { EditCategoryComponent } from './Core/Components/Category/edit-category/edit-category.component';
import { BlogpostListComponent } from './Core/Components/BlogPost/blogpost-list/blogpost-list.component';
import { AddBlogpostComponent } from './Core/Components/BlogPost/add-blogpost/add-blogpost.component';
import { EditBlogpostComponent } from './Core/Components/BlogPost/edit-blogpost/edit-blogpost.component';
import { HomeComponent } from './Core/Components/Public/home/home.component';
import { BlogDetailsComponent } from './Core/Components/Public/blog-details/blog-details.component';
import { LoginComponent } from './Core/Components/Login/login/login.component';
import { adminGuard } from './Guards/admin.guard';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'blog/:url',
        component: BlogDetailsComponent
    },
    {
        path: 'admin/categories/add',
        component: AddCategoryComponent,
        canActivate: [adminGuard]
    },
    {
        // If the user opens the page with "/admin/categories" appended to the end, we will show the category list component
        path: 'admin/categories',
        component: CategoryListComponent,
        canActivate: [adminGuard]
    },
    {
        path: 'admin/categories/:id',
        component: EditCategoryComponent,
        canActivate: [adminGuard]
    },
    {
        path: 'admin/blogposts',
        component: BlogpostListComponent,
        canActivate: [adminGuard]
    },
    {
        path: 'admin/blogposts/add',
        component: AddBlogpostComponent,
        canActivate: [adminGuard]
    },
    {
        path: 'admin/blogposts/:id',
        component: EditBlogpostComponent,
        canActivate: [adminGuard]
    }
    

];