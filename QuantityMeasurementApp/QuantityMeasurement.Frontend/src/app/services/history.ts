import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { AuthService } from './auth';

@Injectable({

providedIn:'root'

})

export class HistoryService{

private api = 'https://quantity-backend.onrender.com/api/history';

constructor(

private http:HttpClient,

private auth:AuthService

){}

getHistory(){

return this.http.get<unknown[]>(

`${this.api}`,

{

headers:this.auth.getAuthHeaders()

}

);

}

clearHistory(){

return this.http.delete<void>(

`${this.api}`,

{

headers:this.auth.getAuthHeaders()

}

);

}

}
