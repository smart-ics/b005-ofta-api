DELETE OFTA_DocType
WHERE IsStandard = 1
GO

INSERT INTO OFTA_DocType
SELECT 'DTX00',  'Merger File Klaim Bpjs', 1, 1, '', '' UNION ALL
SELECT 'DTX01',  'Text Eklaim', 1, 1, '', '' UNION ALL
SELECT 'DTX02',  'SEP', 1, 1, '', '' UNION ALL                                               
SELECT 'DTX03',  'SKDP', 1, 1, '', '' UNION ALL
SELECT 'DTX04',  'SPRI', 1, 1, '', '' UNION ALL
SELECT 'DTX05',  'Resume Medis', 1, 1, '', '' UNION ALL
SELECT 'DTX06',  'Surat Rujukan', 1, 1, '', '' UNION ALL
SELECT 'DTX07',  'Nota Bill', 1, 1, '', 'RO-NOTA-BILL' UNION ALL
SELECT 'DTX08',  'Hasil Radiologi', 1, 1, '', '' UNION ALL
SELECT 'DTX09',  'Hasil Laborat', 1, 1, '', '' UNION ALL
SELECT 'DTX0A',  'Laporan Operasi', 1, 1, '', '' UNION ALL
SELECT 'DTX0B',  'Observasi HD', 1, 1, '', '' UNION ALL
SELECT 'DTX0C',  'Resep', 1, 1, '', 'KP-RESEP' UNION ALL
SELECT 'DTX0D',  'Nota Obat', 1, 1, '', 'DU-NOTA-JUAL' UNION ALL
SELECT 'DTX0E',  'CPPT', 1, 1, '','' 
GO
