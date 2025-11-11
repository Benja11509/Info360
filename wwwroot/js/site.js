function resolverImg(opcion, id){
const rta = document.getElementById("rta").value
const option = document.getElementById(id).value
const respuesta = document.getElementById("rtaFinal")
option.innerHTML = ""
rta.innerHTML = opcion
respuesta.innerHTML = id
}
function vaciarResp(){
    const rta = document.getElementById("rta").value
    rta.innerHTML = "";
    const respuesta = document.getElementById("rtaFinal")
    respuesta.innerHTML = ""
}