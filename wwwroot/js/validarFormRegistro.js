localStorage.clear()

const reValueLabs = /^\*(?!Direccion|Telefono).*/
const reNombre = /^[a-zA-Z]{1,50}$/
const reNIF = /^[0-9]{8}[a-zA-Z]$/
const reCIF = /^[a-zA-Z][0-9]{8}$/
const rePasaporte = /^[0-9]{9}$/
const rePassword = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\$%\^&\*\+])(?=.{8,20})/
const reEmail = /^.*@.*\.(com|es|uk|it|org)$/
const reTlfno = /^[0-9]{9}$/

//estado de los input obligatorios
let estadoCajas = {}

function setEstadoCajasByDefault(id) {
    estadoCajas[id] = false
}

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

function validarCamposEq(inputCampo, inputConfirmCampo, attrLabFor) {
    const campoCubierto = estadoCajas[inputCampo] === true
    const confirmCampoCubierto = estadoCajas[inputConfirmCampo] === true

    if (campoCubierto && confirmCampoCubierto) {
        const campo = $(`#${inputCampo}`).val()
        const confirmCampo = $(`#${inputConfirmCampo}`).val()
        estadoCajas[attrLabFor] = campo === confirmCampo
    }
}

function checkEstadoCajas() {
    const cajasKO = Object.keys(estadoCajas).some(input => estadoCajas[input] === false)

    if (!cajasKO) {
        $('#btnEnviarAlta').removeAttr('disabled')
    }
}

$('label')
    .filter((index, lab) => reValueLabs.test($(lab).text()))
    .each((index, lab) => {
        const attrLabFor = $(lab).attr('for')
        setEstadoCajasByDefault(attrLabFor)

        $('input[id="' + $(lab).attr('for') + '"]').blur(function (ev) {
            const value = $(ev.target).val()
            const id = $(ev.target).attr('id')
            validarSiCampoCubierto({ value, id })

            switch (attrLabFor) {
                case "inputNombre":
                case "inputNombreVia":
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
                    validarCamposEq(attrLabFor)
                    break
                case "inputEmail":
                case "inputConfEmail":
                    validarValorCampo(reEmail, attrLabFor)
                    validarCamposEq(attrLabFor)
                    break
                case "inputTelefono":
                    validarValorCampo(reTlfno, attrLabFor)
                    break
            }
            checkEstadoCajas()
        })
    })