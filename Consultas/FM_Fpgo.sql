
DECLARE @FchDes DateTime
DECLARE @FchHas DateTime

SET @FchDes = '20250913'
SET @FchHas = '20250914'

--SET @FchDes='?FchDes'
--SET @FchHas='?FchHas'

SELECT
	  EncCFM.UsrMdf "Usr"
	, EncCFM.StmMdf "Fecha Hora"
	, EncCFM.CodDoc+'-'+ EncCFM.IdAlf "Doc"
	, CASE 
		WHEN EncCFM.Clt = '99' THEN dbo.Lst_Prm(EncCFM.DatClt,'|') + ' - ' + dbo.Lst_NsmoItm(EncCFM.DatClt,2,'|')
		ELSE EncCFM.Clt + ' - ' + Clt.NomClt END "Cliente"
	, MonDoc
	, EncCFM.TotDoc "Total Doc"
	, dbo.Mnt_CnvExt(TotDoc, MonDoc, 'USD', 'BCCT', FchDoc, 'GE', 2) "TotDoc USD"
	, dbo.Tsa_Obt('BCCT', 'USD', MonDoc, FchDoc,'GE') "Tasa"
	--, EncCFM.LprcDoc "Lista"
	, DscFpgo
	, MontoFpgo
	, dbo.Mnt_CnvExt(MontoFpgo, Mon, 'USD', 'BCCT', FchDoc, 'GE', 2) "TotFpgo USD"
  FROM GE_EncCFM EncCFM
  LEFT JOIN GE_Clt Clt ON Clt.Clt = EncCFM.Clt
  LEFT JOIN (
 SELECT IdDoc, DscFpgo, Mon, SUM(TotMovV) MontoFpgo
 FROM (
		SELECT 
			-1 TipVmt
		   , IdDoc
		   , CASE TipVmt
			   WHEN 10 THEN 'Efectivo'
			   WHEN 30 THEN 'Cheque'
			   WHEN 60 THEN ISNULL(Cpn.NomCpn,'')
			   WHEN 70 THEN ISNULL(Trjp.NomTrjp,'')
			   WHEN 80 THEN 'Concepto'
			   WHEN 90 THEN 'Mov. Bancario'	   
			   ELSE '' END  + ' (' + DetVM.MonVmt +')' DscFpgo
			 , DetVM.MonVmt Mon
			, CASE WHEN DetVM.NcjUbvOri<>0 THEN -1 ELSE 1 END * DetVM.MntMovD TotMov
			, CASE WHEN DetVM.NcjUbvOri<>0 THEN -1 ELSE 1 END * DetVM.MntMovV TotMovV
		FROM GE_DetVM DetVM 
			LEFT JOIN GE_Cpn Cpn
			   ON Cpn.Cpn=DetVM.Cpn
			LEFT JOIN GE_Trjp Trjp
			   ON Trjp.Trjp=DetVM.Trjp
		WHERE DetVM.FchDoc BETWEEN @FchDes AND @FchHas

		UNION ALL
		SELECT 
			-2 TipVmt
			, IdDoc
			, CASE OpMbc
			  WHEN 100 THEN 'Dep�sito'
			  WHEN 504 THEN 'Transferencia Entrada'
			  WHEN 510 THEN 'Transferencia Salida'
			  ELSE CAST(OpMbc AS varchar(10))
			  END + ' ' +Mbc.Ctab + ' (' + Mbc.MonMbc +')' DscFpgo
			, Mbc.MonMbc Mon
			, dbo.Cad_Cur(Z_MntMbcD.VlrACol) TotMov
			, MntMbc TotMovV
			
		FROM GE_DetMB DetMB 
			LEFT JOIN GE_Mbc Mbc
			   ON Mbc.IdMbc=DetMB.IdMbc
			LEFT JOIN GE_VwZItfT Z_MntMbcD
			   ON Z_MntMbcD.NomTbl='GE_DetMB'
			   AND Z_MntMbcD.PkReg=DetMB.IdDoc+'|'+DetMB.IdBlk+'Ã¼'+CAST(DetMB.Sec AS CHAR)
			   AND Z_MntMbcD.NomCol='MntMbcD'
		WHERE DetMB.FchDoc BETWEEN @FchDes AND @FchHas
		UNION ALL
		SELECT 
			-3 TipVmt
			, IdDoc
			, DscDet + ' (' + MonMov +')'
			, MonMov Mon
			, CASE WHEN DetCN.TipDoc=23 THEN -1 ELSE 1 END * DetCN.MntMov TotMov
			, CASE WHEN DetCN.TipDoc=23 THEN -1 ELSE 1 END * DetCN.MntMov TotMovV
		FROM GE_DetCN DetCN
		UNION ALL
		SELECT 
			-4 TipVmt
			, IdDoc 
			, 'A Cuenta' + ' (' + MonDoc +')' DscFpgo
			, MonDoc Mon
			, dbo.Cad_Cur(Z_MntMbcD.VlrACol) TotMov
			, dbo.Cad_Cur(Z_MntMbcD.VlrACol) TotMovV
		  FROM GE_EncCFM CFM	
		  LEFT JOIN GE_VwZItfT Z_MntMbcD
			ON Z_MntMbcD.NomTbl='GE_EncCFM'
			AND Z_MntMbcD.PkReg=CFM.IdDoc 
			AND Z_MntMbcD.NomCol='TotACrdD'
		WHERE FchDoc BETWEEN @FchDes AND @FchHas
		  AND Z_MntMbcD.VlrACol <> '0'
	
	) Vw
	GROUP BY IdDoc, DscFpgo, Mon
) Fpgo 
 ON Fpgo.IdDoc = EncCFM.IdDoc

WHERE EncCFM.FchDoc BETWEEN @FchDes AND @FchHas
  AND EncCFM.StDoc = 1
ORDER BY EncCFM.FchDoc, EncCFM.IdDoc