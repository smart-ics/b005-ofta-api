DELETE OFTA_DocType
WHERE IsStandard = 1
GO

INSERT INTO OFTA_DocType
SELECT 'DX001',  'Text Eklaim', 1, 1 UNION ALL
SELECT 'DX002',  'SEP', 1, 1 UNION ALL                                               
SELECT 'DX003',  'SKDP', 1, 1 UNION ALL
SELECT 'DX004',  'SPRI', 1, 1 UNION ALL
SELECT 'DX005',  'Resume Medis', 1, 1 UNION ALL
SELECT 'DX006',  'Surat Rujukan', 1, 1 UNION ALL
SELECT 'DX007',  'Nota Bill', 1, 1 UNION ALL
SELECT 'DX008',  'Hasil Radiologi', 1, 1 UNION ALL
SELECT 'DX009',  'Hasil Laborat', 1, 1 UNION ALL
SELECT 'DX00A',  'Laporan Operasi', 1, 1 UNION ALL
SELECT 'DX00B',  'Observasi HD', 1, 1 UNION ALL
SELECT 'DX00C',  'Resep', 1, 1 UNION ALL
SELECT 'DX00D',  'Nota OBat', 1, 1 UNION ALL
SELECT 'DX00E',  'CPPT', 1, 1 
GO
