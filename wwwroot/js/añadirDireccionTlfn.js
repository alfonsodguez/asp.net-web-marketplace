let contadorDirecciones = 0
let contadorTlfnos = 0

$('#botonAceptarDirec').click(function (ev) {
    const nuevaDireccion = {}

    $('div[id="altaDireccion"] label').each((index, lab) => {
        const attrLabFor = $(lab).attr('for')
        if (attrLabFor != "inputEsPrincipal") {
            nuevaDireccion[$('#' + attrLabFor).attr('name')] = $('#' + attrLabFor).val()
        } else {
            nuevaDireccion[$('#' + attrLabFor).attr('name')] = $('#' + attrLabFor).is(':checked')
        }
    })

    nuevaDireccion['codProvincia'] = $('#inputLocalidad').attr('value')
    nuevaDireccion['codMunicipio'] = $('#inputLocalidad').attr('label')

    const esOpcionPorDefecto = $('#direcciones > option').first().val() === "No se han definido direcciones"
    if (esOpcionPorDefecto) {
        $('#direcciones > option').first().remove()
    }

    const textoDireccionDropdown = `${nuevaDireccion.tipoVia} ${nuevaDireccion.nombreVia}, ${nuevaDireccion.numeroVia} ${nuevaDireccion.piso}-${nuevaDireccion.puerta} CP: ${nuevaDireccion.cp}`
    if (esDireccionModif) {
        localStorage.setItem('direc-' + esDireccionModif, JSON.stringify(nuevaDireccion))
        $(`#direcciones option[value="${posTlfnModif}"]`).text(textoDireccionDropdown)
    } else {
        localStorage.setItem('direc-' + contadorDirecciones, JSON.stringify(nuevaDireccion))
        $('#direcciones').append(`<option value="${contadorDirecciones}">${textoDireccionDropdown}</option>`)
        contadorDirecciones++
    }

    $('#botonModificarDirec, #botonBorrarDirec').removeAttr('disabled')
    modalAltaDirecccion.hide()
})

$('#botonAceptarTlfno').click(function (ev) {
    const nuevoTlfno = {
        numero: $('#inputTelefono').val(),
        esPrincipal: $('#esTlfnoprincipal').is(':checked')
    }

    const esOpcionPorDefecto = $('#telefonos > option').first().val() === "No se han definido telefonos"
    if (esOpcionPorDefecto) {
        $('#telefonos > option').first().remove()
    }

    if (esTlfnModif) {
        localStorage.setItem('tlfno-' + esTlfnModif, JSON.stringify(nuevoTlfno))
        $(`#telefonos option[value="${posTlfnModif}"]`).text(nuevoTlfno.numero)
    } else {
        localStorage.setItem('tlfno-' + contadorTlfnos, JSON.stringify(nuevoTlfno))
        $('#telefonos').append(`<option value="${contadorTlfnos}">${nuevoTlfno.numero}</option>`)
        contadorTlfnos++
    }

    $('#botonModififcarTelef, #botonBorrarTelef').removeAttr('disabled')
    modalAltaTlfn.hide()
})
