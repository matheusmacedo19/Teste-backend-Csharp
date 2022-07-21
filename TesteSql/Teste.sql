--TESTE 1
SELECT stts.dsStatus, COUNT(process.IdProcesso)AS qtdprocesso 
FROM tb_Processo AS process
LEFT JOIN tb_Status AS stts ON (process.idStatus = stts.idStatus)
GROUP BY 
stts.dsStatus


--TESTE 2
SELECT process.nroProcesso, MAX(andmnt.dtAndamento) AS MaiorData 
FROM tb_Andamento AS andmnt
INNER JOIN tb_Processo AS process ON (andmnt.idProcesso = process.idProcesso)
WHERE process.DtEncerramento = '2013-01-01'
GROUP BY
process.nroProcesso

--TESTE 3
SELECT COUNT(DtEncerramento) FROM tb_Processo
WHERE (SELECT COUNT(DtEncerramento) FROM tb_Processo) > 5

--TESTE 4
SELECT REPLICATE('0', 12 - LEN([nroProcesso])) + RTRIM([nroProcesso]) AS nroProcesso12Char 
FROM tb_Processo
