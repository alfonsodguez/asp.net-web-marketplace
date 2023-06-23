localStorage.clear()
//estado de los input obligatorios
let estadoCajas = { 'direcciones': false, 'telefonos': false }

const reValueLabs = /^\*(?!Direccion|Nombre via|Telefono).*/
const reNombre    = /^[a-zA-Z]{1,50}$/
const reNIF       = /^[0-9]{8}[a-zA-Z]$/
const reCIF       = /^[a-zA-Z][0-9]{8}$/
const rePasaporte = /^[0-9]{9}$/
const rePassword  = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*])(?=.{8,20})/
const reEmail     = /^.*@.*\.(com|es|uk|it|org)$/

function validarValorCampo(patron, id) {
    const inputId = $('#' + id).val()
    const isValid = patron.test(inputId)

    if (isValid) {
        estadoCajas[id] = true
    } else {
        estadoCajas[id] = false
    }
}

function validarSiCampoCubierto({ value, id }) {
    const campoCubierto = value === undefined || value === ''

    if (campoCubierto) {
        estadoCajas[id] = false
    } else {
        estadoCajas[id] = true
    }
}

function validarDireccionTlfno() {
    const numDireccionesStorage = 0
    const numTelefonosStorage   = 0

    for (let a = 0; a < localStorage.length; a++) {
        if (/^direc-/.test(localStorage.key(a))) {
            numDireccionesStorage++
        }
        if (/^tlfno-/.test(localStorage.key(a))) {
            numTelefonosStorage++
        }
    }

    const esDireccionesOk = $('#direcciones option').length === numDireccionesStorage
    const esTelefonosOk   = $('#telefonos option').length === numTelefonosStorage

    estadoCajas['direcciones'] = esDireccionesOk
    estadoCajas['telefonos']   = esTelefonosOk
}

function setEstadoCajasByDefault(id) {
    estadoCajas[id] = false
}

function setEstadoCajaPassword() {
    const passwordCubierto  = estadoCajas['inputPassword'] === true
    const repassworCubierto = estadoCajas['inputRePassword'] === true

    if (passwordCubierto && repassworCubierto) {
        const password   = $('#inputPassword').val()
        const repassword = $('#inputRePassword').val()

        estadoCajas[attrLabFor] = password === repassword
    }
}

function setEstadoCajaEmail() {
    const email = $('#inputEmail').val()
    const confirmarEmail = $('#inputConfEmail').val()

    estadoCajas[attrLabFor] = email === confirmarEmail
}

function checkEstadoCajas() {
    const cajasOk = Object.keys(estadoCajas).some(input => estadoCajas[input] === false)

    if (cajasOk) {
        $('#btnEnviarAlta').attr('disabled', 'true')
    }
    $('#btnEnviarAlta').removeAttr('disabled')
}


$('label')
.filter((index, lab) => reValueLabs.test($(lab).text()))
.each((index, lab) => {
    const attrLabFor = $(lab).attr('for')
    setEstadoCajasByDefault(attrLabFor)

    $('input[id="' + $(lab).attr('for') + '"]').blur(function (ev) {
        const value = $(ev.target).val()
        const id    = $(ev.target).attr('id')
        validarSiCampoCubierto({ value, id })

        switch (attrLabFor) {
            case "inputNombre":
                validarValorCampo(reNombre, attrLabFor)
                break
            case "inputIdentif":
                switch ($('#dropdownTipoIdentif').val()) {
                    case 'NIF':
                        validarValorCampo(reNIF, attrLabFor)
                        break
                    case 'CIF':
                        validarValorCampo(reCIF, attrLabFor)
                        break
                    case 'Pasaporte':
                        validarValorCampo(rePasaporte, attrLabFor)
                        break
                }
                break
            case "inputPassword":
            case "inputRePassword":
                validarValorCampo(rePassword, attrLabFor)
                setEstadoCajaPassword()
                break
            case "inputEmail":
            case "inputConfEmail":
                validarValorCampo(reEmail, attrLabFor)
                setEstadoCajaEmail()
                break
        }

        validarDireccionTlfno()
        checkEstadoCajas()
    })
})