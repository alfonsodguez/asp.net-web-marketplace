$('button:submit').click(function (evt) {
    evt.preventDefault()
    // diccionarios 
    const direcciones = {}
    const telefonos = {}

    for (let pos=0; pos<localStorage.length; pos++) {
        // tenemos almacenado direcciones y telefonos con las keys: "direct-0", "tlfno-0", ....
        const key = localStorage.key(pos)
        const value = JSON.parse(localStorage[key])
        
        if (/^direc-/.test(key)) {
            direcciones[key] = value
        }
        else if (/^tlfno-/.test(key)) {
            telefonos[key] = value
        }
    };

    const cliente = {
        nombre: $('#inputNombre').val(),
        primerApellido: $('#inputPrimApe').val(),
        segundoApellido: $('#inputSecApe').val(),
        fechaNacimiento: $('#inputFecha').val(),
        tipoIdentificacionCliente: {
            tipoId: $('select[name="tipoId"]').val(),
            numeroId: $('#inputIdentif').val(),
        },
        credencialesCliente: {
            email: $('#inputEmail').val(),
            password: $('#inputPassword').val(),
        },
        telefonos,
        direcciones,
    }

    $.ajax({
        type: 'POST',
        url: 'https://localhost:44329/Cliente/Registro',
        data: JSON.stringify(cliente),
        contentType: 'application/json',
        processData: false
    })
    .done((res) => {
        console.log('cliente registrado ok')
    })
    .fail((err) => {
        console.log(err)
    })
})