# OFTA (Office Automation) Documentation

1. Deskripsi Produk

   1.1. Goal
   OFTA adalah system software untuk mengubah pencatatan dan penyimpanan dokumen rumah sakit dari menggunakan kertas menjadi dokumen digital.

   1.2. Objective 1. Memindahkan sistem pencatatan dokumen template dari EMR-1 dan EMR-2 ke OFTA 2. Menerapkan TTE Tersertifikasi dalam proses Approval Document 3. Bundling Document untuk lampiran klaim BPJS

2. Specification

   2.2. Use-Case 1. Pegawai membutuhkan approval atas sebuah dokumen - Agus: Pegawai - Budi: Kepala Bagian (Sign-1) - Candra: Kepala HRD (Sign-2)

````mermaid
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
```mermaid

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
```mermaid


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
```mermaid

```mermaid
sequenceDiagram
    title SKENARIO-4: Bundling Document By Catalog

    participant gugun
    participant OFTA
    participant REMOTECETAK
    participant TEKENAJA
    participant hanum
    participant indah

    Note right of gugun: gugun: petugas klaim bpjs
    Note right of hanum: hanum: dokter lab
    Note right of indah: indah: dokter dpjp

    OFTA ->> gugun : 1. Request Catalog Klaim BPJS
    gugun ->> gugun: 2. Input Catalog Header (Data Pasien)
    OFTA -->> gugun: 3. Download List Document Required by Catalog


    gugun ->> REMOTECETAK: 4. Print Unlisted Document: Hasil Lab
    REMOTECETAK -->> OFTA: 5. Submit Hasil Lab
    OFTA -->> TEKENAJA: 6. Upload Hasil Lab
    TEKENAJA -->> hanum: 7. Notif Sign
    hanum ->> TEKENAJA: 8. Sign HasilLab
    TEKENAJA -->> OFTA : 9. CallBack HasilLab has been signed
    OFTA -->> gugun: 10. Notif HasilLab has been signed
    OFTA ->> gugun: 11. Dowload HasilLab


    gugun ->> OFTA: 13.1 Request Template Surat Selesai Rawat
    OFTA -->> gugun: 13.2 Download Template Surat Selesai Rawat
    gugun ->> gugun: 14. Lengkapi Template Surat Selesai Rawat
    gugun ->> OFTA: 15. Submit Surat Selesai Rawat
    OFTA -->> TEKENAJA: 16. Upload Document
    TEKENAJA -->> indah: 17. Notif Sign Document
    indah ->> TEKENAJA: 18. Sign Document
    TEKENAJA -->> OFTA: 19. CallBack Surat Selesai Rawat has been signed
    OFTA -->> gugun: 20. Notif Surat Selesai Rawat has been signed
    gugun ->> OFTA: 21.1 Request Download Surat Selesai Rawat
    OFTA -->> gugun: 21.2 Dowload Surat Selesai Rawat
```mermaid

```mermaid
erDiagram
    TEMPLATE{
        string TemplateId PK
        string TemplateName
        string Url
    }

    DOCUMENT-TYPE{
        string DocumentTypeId PK
        string DocumentTypeName
        string TemplateId
    }

    DOCUMENT{
        string DocumentId PK
        datetime CreateDate
        string UserId
        string DocumentTypeId
        bool IsPublished
        datetime PublishedDate
        bool IsArchieved
        datetime ArchievedDate
    }

    DOCUMENT-SIGNEE{
        string UserId
        int SignPosition
        datetime SignDate
        bool IsSigned
    }

    USER{
        string UserId PK
        string Email
        bool IsVerified
        datetime VerifiedDate
    }

    CATALOG{
        string CatalogId PK
        string CatalogName
    }

    CATALOG-DOCTYPE{
        string CatalogId
        string DocumentTypeId
        int NoUrut
    }

    BUNDLING{
        string BundlingId PK
        string CatalogId
        datetime BundlingDate
        bool IsPublished
        datetime PublishedDate
    }

    BUNDLING-DOC{
        string BundlingId
        string DocumentId
        int NoUrut
    }

    DOCUMENT-TYPE ||--o| TEMPLATE : has
    DOCUMENT ||--|{ DOCUMENT-SIGNEE : contains
    DOCUMENT-SIGNEE }|--|| USER : is

    CATALOG ||--|{ CATALOG-DOCTYPE : contains

    BUNDLING ||--|{ BUNDLING-DOC : contains
    --BUNDLING }|--|| CATALOG : based-on
    --DOCUMENT }|--|| DOCUMENT-TYPE : based-on
    --BUNDLING-DOC ||--|| DOCUMENT : is
    --CATALOG-DOCTYPE }|--|| DOCUMENT-TYPE : is
```mermaid

````
