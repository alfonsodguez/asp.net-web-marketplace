// TODO: modificar y borrar tlfnos
$('#botonModififcarDirec').click(function (ev) {
    modalAltaDir.show()
    const numDirecciones = $('#direcciones').val()
    const direccionStorage = JSON.parse(localStorage.getItem('direc-' + numDirecciones))

    esDireccionModif = true
    posDireccionModif = parseInt(numDirecciones)

    Object.keys(direccionStorage).forEach((prop) => {
        switch (prop) {
            case 'tipoVia':
            case 'codpro':
            case 'codmun':
                $(`select[name="${prop}"]`).val(direccionStorage[prop])
                if (prop == 'codpro') {
                    document.getElementById('#inputProvincia').dispatchEvent('change')
                }
                break
            default:
                $(`input[name="${prop}"]`).val(direccionStorage[prop])
                break
        }
    })
})