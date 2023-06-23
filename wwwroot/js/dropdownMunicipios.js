$('#inputProvincia').change(function (evt) {
    $('#inputLocalidad > option[value != 0]').remove()

    const codProvincia = evt.target.value
    const urlMunicipios = 'https://localhost:44329/api/REST/DevolverMunicipios?codpro=' + codProvincia

    if (codProvincia != "0") {
        $('#inputLocalidad').removeAttr('disabled')

        $
        .get(urlMunicipios)
        .done((municipios) => {
            municipios.forEach((municipio) => {
                $('#inputLocalidad').append('<option value=' + municipio.codMunicipio + 'label=' + codProvincia + '>' + municipio.nombre + '</option>')
            })
        })
        .fail((err) => { console.log(err) })
    }
})