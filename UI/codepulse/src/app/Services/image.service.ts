import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Image } from '../Models/Image/image.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ImageService {

    selectedImage: BehaviorSubject<Image> = new BehaviorSubject<Image>({
        id: '',
        fileExtension: '',
        fileName: '',
        url: '',
        title: ''
    });

    constructor(private http: HttpClient) { }

    uploadImage(file: File, fileName: string, title: string): Observable<Image>{
        const formData = new FormData();
        formData.append('file', file);
        formData.append('fileName', fileName);
        formData.append('title', title);

        return this.http.post<Image>(`${environment.apiBaseUrl}/api/image`, formData);
    }

    getAll(): Observable<Image[]>{
        return this.http.get<Image[]>(`${environment.apiBaseUrl}/api/image`);
    }

    selectImage(image: Image): void{
        this.selectedImage.next(image);
    }

    onSelectImage(): Observable<Image>{
        return this.selectedImage.asObservable();
    }

}
