const modalAltaDirecccion = new bootstrap.Modal(document.getElementById('formAltaDirec'));
$('#botonAltaDirec').click(function () { modalAltaDirecccion.show(); });
$('#botonCierraModal').click(function () { modalAltaDirecccion.hide(); });

const modalAltaTlf = new bootstrap.Modal(document.getElementById('formAltaTelef'));
$('#botonAltaTelef').click(function () { modalAltaTlf.show(); });
$('#botonCierraModalTlfno').click(function () { modalAltaTlf.hide(); });