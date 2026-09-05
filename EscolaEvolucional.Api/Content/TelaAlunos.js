(function ($) {
    "use strict";
    var paginaAtual = 1;
    var tamanhoPagina = 10;
    var totalRegistros = 0;
    var carregando = false;

    function textoSeguro(valor) { return $("<div>").text(valor || "").html(); }
    function formatarData(valor) {
        var data = valor ? new Date(valor) : null;
        return !data || isNaN(data.getTime()) ? "-" : data.toLocaleDateString("pt-BR");
    }
    function atualizarPaginacao() {
        var totalPaginas = Math.max(1, Math.ceil(totalRegistros / tamanhoPagina));
        $("#resumo-paginacao").text("Página " + paginaAtual + " de " + totalPaginas + " — " + totalRegistros + " registro(s)");
        $("#pagina-anterior").prop("disabled", carregando || paginaAtual <= 1);
        $("#proxima-pagina").prop("disabled", carregando || paginaAtual >= totalPaginas);
    }
    function exibirMensagem(texto, erro) { $("#mensagem").text(texto).toggleClass("erro", !!erro); }
    function renderizarAlunos(alunos) {
        var linhas = "";
        $.each(alunos, function (_, aluno) {
            var situacao = aluno.ativo ? "Ativo" : "Inativo";
            var classeSituacao = aluno.ativo ? "ativo" : "inativo";
            linhas += "<tr><td>" + textoSeguro(aluno.nome) + "</td><td>" + textoSeguro(aluno.email) + "</td><td>" + formatarData(aluno.dataNascimento) + "</td><td><span class=\"status " + classeSituacao + "\">" + situacao + "</span></td></tr>";
        });
        $("#alunos").html(linhas);
    }
    function carregarAlunos() {
        if (carregando) { return; }
        carregando = true;
        exibirMensagem("Carregando alunos...");
        atualizarPaginacao();
        $.ajax({
            url: "/api/alunos", method: "GET", dataType: "json",
            data: { nome: $.trim($("#nome").val()), page: paginaAtual, pageSize: tamanhoPagina }
        }).done(function (resultado) {
            var alunos = resultado.items || [];
            totalRegistros = resultado.total || 0;
            paginaAtual = resultado.page || paginaAtual;
            renderizarAlunos(alunos);
            exibirMensagem(alunos.length === 0 ? "Nenhum aluno encontrado." : "");
        }).fail(function () {
            totalRegistros = 0;
            renderizarAlunos([]);
            exibirMensagem("Não foi possível consultar os alunos. Verifique se a API está em execução.", true);
        }).always(function () {
            carregando = false;
            atualizarPaginacao();
        });
    }
    $(function () {
        $("#filtro-alunos").on("submit", function (evento) { evento.preventDefault(); paginaAtual = 1; carregarAlunos(); });
        $("#pagina-anterior").on("click", function () { if (paginaAtual > 1) { paginaAtual--; carregarAlunos(); } });
        $("#proxima-pagina").on("click", function () { if (paginaAtual * tamanhoPagina < totalRegistros) { paginaAtual++; carregarAlunos(); } });
        atualizarPaginacao();
        carregarAlunos();
    });
}(jQuery));