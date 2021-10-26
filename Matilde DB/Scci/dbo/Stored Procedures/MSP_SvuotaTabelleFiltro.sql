CREATE PROCEDURE MSP_SvuotaTabelleFiltro
AS
BEGIN
	select count(*) from T_TmpFiltriAllegati
	select count(*) from T_TmpFiltriAppuntamenti
	select count(*) from T_TmpFiltriAppuntamentiAgende
	select count(*) from T_TmpFiltriDiario
	select count(*) from T_TmpFiltriEpisodi
	select count(*) from T_TmpFiltriNoteAgende
	select count(*) from T_TmpFiltriOrdini
	select count(*) from T_TmpFiltriParametriVitali
	select count(*) from T_TmpFiltriPrescrizioni
	select count(*) from T_TmpFiltriPrescrizioniTempi
	select count(*) from T_TmpFiltriSchede
	select count(*) from T_TmpFiltriTaskInfermieristici

	TRUNCATE TABLE T_TmpFiltriAllegati
	TRUNCATE TABLE T_TmpFiltriAllegati
	TRUNCATE TABLE T_TmpFiltriAppuntamenti
	TRUNCATE TABLE T_TmpFiltriAppuntamentiAgende
	TRUNCATE TABLE T_TmpFiltriDiario
	TRUNCATE TABLE T_TmpFiltriEpisodi
	TRUNCATE TABLE T_TmpFiltriNoteAgende
	TRUNCATE TABLE T_TmpFiltriOrdini
	TRUNCATE TABLE T_TmpFiltriParametriVitali
	TRUNCATE TABLE T_TmpFiltriPrescrizioni
	TRUNCATE TABLE T_TmpFiltriPrescrizioniTempi
	TRUNCATE TABLE T_TmpFiltriSchede
	TRUNCATE TABLE T_TmpFiltriTaskInfermieristici

END