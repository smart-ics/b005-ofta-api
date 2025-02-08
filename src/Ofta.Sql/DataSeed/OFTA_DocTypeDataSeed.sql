DELETE OFTA_DocType
WHERE IsStandard = 1
GO

INSERT INTO OFTA_DocType
SELECT 'DTX00', 'Merger File Klaim Bpjs', 1, 1, '<url_ofta_storage_lokasi>', '', '', '' UNION ALL
SELECT 'DTX01', 'Text Eklaim', 1, 1, '<url_ofta_storage_lokasi>', 'RG-TXT', '', '' UNION ALL
SELECT 'DTX02', 'SEP', 1, 1, '<url_ofta_storage_lokasi>', 'JP-SEP', '', '' UNION ALL                                               
SELECT 'DTX03', 'SKDP', 1, 1, '<url_ofta_storage_lokasi>', 'RG-SKDP', '', '' UNION ALL
SELECT 'DTX04', 'SPRI', 1, 1, '<url_ofta_storage_lokasi>', 'RG-SPRI', '', '' UNION ALL
SELECT 'DTX05', 'Resume Administratif', 1, 1, '<url_ofta_storage_lokasi>', 'printResume', 'Resume', '' UNION ALL
SELECT 'DTX06', 'Surat Rujukan', 1, 1, '<url_ofta_storage_lokasi>', 'RG-RUJUKAN', '', '' UNION ALL
SELECT 'DTX07', 'Nota Bill', 1, 1, '<url_ofta_storage_lokasi>', 'RO-REKAP', '', '' UNION ALL
SELECT 'DTX08', 'Hasil Radiologi', 1, 1, '<url_ofta_storage_lokasi>', 'TU-RAD', '', '' UNION ALL
SELECT 'DTX09', 'Hasil Laborat', 1, 1, '<url_ofta_storage_lokasi>', 'TU-LAB', '', '' UNION ALL
SELECT 'DTX0A', 'Laporan Operasi', 1, 1, '<url_ofta_storage_lokasi>', '', '', '' UNION ALL
SELECT 'DTX0B', 'Observasi HD', 1, 1, '<url_ofta_storage_lokasi>', '', '', '' UNION ALL
SELECT 'DTX0C', 'Resep', 1, 1, '<url_ofta_storage_lokasi>', 'KP-RESEP', '', '' UNION ALL
SELECT 'DTX0D', 'Nota Obat', 1, 1, '<url_ofta_storage_lokasi>', 'DU-NOTA-JUAL', '', '' UNION ALL
SELECT 'DTX0E', 'CPPT', 1, 1, '<url_ofta_storage_lokasi>', 'printCppt', 'Cppt', '' UNION ALL
SELECT 'DTX0F', 'Ass Awal Medis IGD', 1, 1, '<url_ofta_storage_lokasi>', 'printSmass', 'PP-ICS-000F' , '' UNION ALL
SELECT 'DTX10', 'Hasil Lab Luar', 1, 1, '<url_ofta_storage_lokasi>', 'printOtherDoc', '06' , '' UNION ALL
SELECT 'DTX11', 'Hasil Rad Luar', 1, 1, '<url_ofta_storage_lokasi>', 'printOtherDoc', '08' , '' UNION ALL
SELECT 'DTX12', 'Hasil PA Lab Luar', 1, 1, '<url_ofta_storage_lokasi>', 'printOtherDoc', '11' , '' UNION ALL
SELECT 'DTX13', 'Other Doc', 1, 1, '<url_ofta_storage_lokasi>', 'printOtherDoc', '14', '' UNION ALL
SELECT 'DTX14', 'Resume Medis', 1, 1, '<url_ofta_storage_lokasi>', 'printResume', 'Resume', '' UNION ALL
SELECT 'DTX15', 'Assesment', 1, 1, '<url_ofta_storage_lokasi>', '', 'AS001', '' UNION ALL
SELECT 'DTX16', 'SBPK', 1, 1, '<url_ofta_storage_lokasi>', 'printSbpk', 'Sbpk', ''
GO
