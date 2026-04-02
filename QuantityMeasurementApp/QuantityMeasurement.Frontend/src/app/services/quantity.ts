import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AuthService } from './auth';

@Injectable({

providedIn:'root'

})

export class QuantityService{

private api = 'https://quantity-backend.onrender.com/api/quantity';

constructor(

private http:HttpClient,

private auth:AuthService


){}

runOperation(

endpoint:string,

body:Record<string, unknown>

):Observable<unknown>{

return this.http.post<unknown>(

`${this.api}/${endpoint}`,

body,

{

headers:this.auth.getAuthHeaders()

}

);

}

}
