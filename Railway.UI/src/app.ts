let el = this.document.getElementById("content");
 
class User{
    name : string;
    age : number;
    constructor(_name:string, _age: number){
         
        this.name = _name;
        this.age = _age;
    }
}
let tom : User = new User("Tom", 31);
el.innerHTML="Name: " + tom.name + " age: " + tom.age;