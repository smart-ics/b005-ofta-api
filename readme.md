# OFTA (Office Automation) Documentation

1. Deskripsi Produk

   1.1. Goal
   OFTA adalah system software untuk mengubah pencatatan dan penyimpanan dokumen rumah sakit dari menggunakan kertas menjadi dokumen digital.

   1.2. Objective
      1. Memindahkan sistem pencatatan dokumen template dari EMR-1 dan EMR-2 ke OFTA
      2. Menerapkan TTE Tersertifikasi dalam proses Approval Document
      3. Bundling Document untuk lampiran klaim BPJS

3. Specification

   2.2. Use-Case 1. Pegawai membutuhkan approval atas sebuah dokumen - Agus: Pegawai - Budi: Kepala Bagian (Sign-1) - Candra: Kepala HRD (Sign-2)

```mermaid
sequenceDiagram
    title SKENARIO-1: Request From External App

    participant agus
    participant HRD
    participant REMOTECETAK
    participant OFTA
    participant TEKENAJA
    participant budi
    participant candra

    Note right of agus: agus: pegawai
    Note right of budi: budi: kepala bagian
    Note right of candra: candra: kepala hrd
    
    agus ->> HRD: 1. Input(Trs.Cuti)
    HRD -->> REMOTECETAK: 2. Print(Trs.Cuti) To PDF
    REMOTECETAK -->> OFTA: 3. Submit(PDF)
    OFTA -->> TEKENAJA: 4. Upload(PDF)
    TEKENAJA -->> budi: 5. Notif() Req.Sign
    budi ->> TEKENAJA: 6. Sign() Document
    TEKENAJA -->> candra: 7. Send() Notif
    candra ->> TEKENAJA: 8. Sign() Doc
    TEKENAJA -->> OFTA: 9. Callback Notif() Signed Doc
    OFTA -->> TEKENAJA: 10.1 Request() Download Signed Doc
    TEKENAJA -->> OFTA: 10.2 Send Signed Document
    OFTA -->> agus: 11. Notif Trs.Cuti Sudah Approved
    OFTA -->> OFTA: 12. Archieve Signed Document
```

```mermaid
sequenceDiagram
    title SKENARIO-2: Request From Template Surat

    participant agus
    participant OFTA
    participant TEKENAJA
    participant budi
    participant candra

    Note right of agus: agus: pegawai
    Note right of budi: budi: kepala bagian
    Note right of candra: candra: kepala hrd

    agus ->> OFTA: 1.1 Request Template Doc Surat Cuti
    OFTA -->> agus: 1.2 Downloaded Template
    agus ->> agus: 2. Isi template document
    agus -->> OFTA: 3. Submit Document
    OFTA -->> TEKENAJA: 4. Upload Document
    TEKENAJA -->> budi: 5. Send Notif
    budi ->> TEKENAJA: 6. Sign Document
    TEKENAJA -->> candra: 7. Send Notif
    candra ->> TEKENAJA: 8. Sign Document
    TEKENAJA -->> OFTA: 9. Notif Signed Document
    OFTA -->> TEKENAJA: 10.1 Request Download Signed Document
    TEKENAJA -->> OFTA: 10.2 Send Signed Document
    OFTA -->> agus: 11. Notif Trs.Cuti Sudah Approved
    OFTA -->> OFTA: 12. Archieve Signed Document
```


```mermaid
sequenceDiagram
    title SKENARIO-3: Request By Unregistered User

    participant deny
    participant TEKENAJA
    participant eka
    participant OFTA
    participant fajar

    Note right of deny: deny: pasien
    Note right of eka: eka: user admisi
    Note right of fajar: fajar: dokter dpjp

    deny ->> TEKENAJA: 1. Registrasi Teken Aja
    eka ->> OFTA: 2.1 Request Template Doc Inform Concent
    OFTA -->> eka: 2.2 Downloaded Template
    eka ->> OFTA: 3. Submit Document
    OFTA -->> TEKENAJA: 4. Upload Document
    TEKENAJA -->> deny: 5. Send Notif
    deny ->> TEKENAJA: 6. Sign Document
    TEKENAJA -->> fajar: 7. Send Notif
    fajar ->> TEKENAJA: 8. Sign Document
    TEKENAJA -->> OFTA: 9. Notif Signed Document
    OFTA -->> TEKENAJA: 10.1 Request Download Signed Document
    TEKENAJA -->> OFTA: 10.2 Send Signed Document
    OFTA -->> OFTA: 11. Archieve Signed Document
```

```mermaid
sequenceDiagram
    title BUNDLING DOCUMENT BPJS Revisi-3, Part-1
    actor kasir
    participant OFTA
    actor casemix
    participant REMOTE_CETAK
    actor layanan
    participant FO/EMR
    autonumber 1

    note over kasir, casemix: EVENT-A [Done]
    kasir ->> OFTA: Order KlaimBpjs 
    OFTA -->> casemix: List Order KlaimBpjs 

    note over OFTA, casemix: EVENT-B [Done]
    casemix ->> OFTA: Create KlaimBpjs
    OFTA -->> casemix: List DocType

    note over casemix: EVENT-C [Done]
    casemix ->> casemix: Sorting ListedDoc
    
    note over casemix, REMOTE_CETAK: EVENT-D
    casemix ->> REMOTE_CETAK: Print SortedDoc
    REMOTE_CETAK -->> casemix : PrintedDoc

    note over OFTA, layanan: EVENT-E
    casemix ->> OFTA: Order Create IncompleteDoc 
    OFTA -->> layanan: Notif Create IncompleteDoc
    
    note over REMOTE_CETAK, FO/EMR: EVENT-F
    layanan ->> FO/EMR : Create IncompleteDoc
    FO/EMR -->> REMOTE_CETAK: Print Doc

    note over OFTA, REMOTE_CETAK: EVENT-G
    loop Loop Until All IncompleteDoc Printed
        OFTA ->> REMOTE_CETAK : Cek Print IncompleteDoc
    end
    
    note over OFTA, casemix: EVENT-H
    OFTA ->> casemix : Notif Doc Completed

    note over OFTA, layanan: EVENT-J
    casemix ->> OFTA: Order Sign PrintedDoc
    OFTA -->> layanan: Notif Sign PrintedDoc


    note over OFTA, layanan: EVENT-K
    loop Loop Until All PrintedDoc Signed
        layanan ->> OFTA: Sign PrintedDoc
    end

    note over OFTA, casemix: EVENT-L
    OFTA ->> casemix: Sign PrintedDoc Completed

    note over OFTA, casemix: EVENT-M
    casemix ->> OFTA: Merge Document
```
```
- mapping event => use-case
  - Event-A => CreateOrderKlaimBpjsCommand;
  - Event-B => CreateKlaimBpjsCommand;
  - Event-C => AddDocTypeToKlaimBpjsCommand, RemoveDocTypeFromKlaimBpjsCommand;
  - Event-D => PrintDocKlaimBpjsCommand;
```

```mermaid
sequenceDiagram
    title Skenario : Tidak Upload Tekenaja
    actor PETUGAS
    participant OFTA
    participant REMOTE CETAK
    participant TEKEN AJA
    autonumber

    PETUGAS ->> OFTA : Request Template Surat
    OFTA -->> PETUGAS: Template Surat
    loop
    PETUGAS ->> PETUGAS : Compose Surat
    end
    PETUGAS ->> OFTA: Submit Surat
    OFTA ->>REMOTE CETAK : Request Cetak Surat
    PETUGAS ->> OFTA : Download Surat

```
