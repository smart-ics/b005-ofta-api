# USE-CASE

1. DocModel
    1. CreateDoc
        - Command:
            - DocTypeId
            - UserId
        - Handler :
            1. Create DocModel
            2. Set DocyType and User sesuai command
            3. Set DocState = Created
            4. AddJurnal(Created)
            5. Write DocModel
        - Response: DocId
        - Event Subscriber [none]

    2. AddSignee
        - Command
            - DocId
            - UserId
            - SignTag
            - SignPosition
        - Handler
            1. Guard: DocState harus 'Created'
            2. Load DocModel
            3. Add Signee sesuai command
            4. Write DocModel
        - Response [none]
        - Event Subscriber [none]

    3. RemoveSignee
        - Command
            - DocId
            - UserId
        - Handler
            1. Guard: DocState harus 'Created'
            2. Load DocModel
            3. RemoveSignee sesuai command
        - Response [none]
        - Event Subscriber [none]

    4. GenDoc
        - Command
            - DocId
        - Handler
            0. Guard DocState harus 'Created'
            1. Load DocId
            2. Set RequestedDocUrl
            3. Write DocModel
            4. Execute GenDocWorker ke RequestedDocUrl
        - Response
            - RequestedDocUrl
        - Event Subscriber

    5. Submit
        - Command
            - DocId
        - Handler
            0. Guard DocState harus 'Created'
            1. Load DocModel
            2. Set DocState = 'Submited'
            3. Set RequestedDocUrl
            4. AddJurnal(Submited)
            5. Write DocModel
        - Response [none]
        - Event Subscriber
            1. SaveFileWorker(RequestedDocUrl)

    6. Upload
        - Command
            - DocId
        - Handler
            0. Guard DocState harus 'Submited'
            1. Load DocModel
            2. Execute UploadTekenAjaWorker
            3. Set UploadedDocId
            4. Set DocState = 'Uploaded'
            5. AddJurnal(Uploaded)
            6. Write DocModel
        - Response [none]
        - Event Subscriber [none]

    7. Signed
        - Command
            - UploadedDocId
            - Signee
        - Handler
            0. Convert UploadedDocId => DocId
            1. Load DocId
            2. Update Sign State sesuai command
            3. Update DocState = "Signed"
            4. AddJurnal(Signed)
            5. Write DocModel
        - Response [none]
        - Event Subscriber [none]
        - Note: Command ini di-trigger ketika ada callback signed document dari tekenAja!

    8. Publish
        - Command
            - UploadedDocId
            - DownloadUrl
        - Handler
            1. Convert UploadedDocId => DocId
            2. Load DocId
            3. UploadedDocUrl = DownloadUrl
            4. PublishedDocUrl = Generate PublishedDocUrl
            5. Update DocState = "Published"
            6. AddJurnal(Published)
            7. Writer DocModel
            8. Execute DownloadUploadedDocWorke; Simpan ke PublishedDocUrl
        - Resnponse [none]
        - Event Subscriber
            - NotifDocOwnerWorker
        - Note: Command ini di-trigger ketika ada callback Complete Signed Document dari tekenAja!
