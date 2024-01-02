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
            4. Write DocModel
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

    4. SetField
        - Command
            - DocId
            - FieldName
            - FieldValue
        - Handler
            1. Guard: DocState harus 'Created'
            2. Load DocModel
            3. SetField susuai command
        - Response [no-response]
        - Event Subscriber [none]

    5. ClearField
        - Command
            - DocId
            - FieldName
        - Handler
            0. Guard DocState harus 'Created'
            1. Load DocModel
            2. Replace field value dengan string.EMpty
            3. Write DocModel
        - Response [none]
        - Event Subscriber [none]

    6. Submit
        - Command
            - DocId
        - Handler
            0. Guard DocState harus 'Created'
            1. Load DocModel
            2. Set DocState = 'Submited'
            3. Write DocModel
        - Response [none]
        - Event Subscriber
            1. SaveFileWorker

    7. Upload
        - Command
            - DocId
        - Handler
            0. Guard DocState harus 'Submited'
            1. Load DocModel
            2. Execute UploadTekenAjaWorker
            3. Set CertifiedDocId
            4. Set DocState = 'Uploaded'
            5. Write DocModel
        - Response [none]
        - Event Subscriber [none]

    8. Signed
        - Command
            - CertifiedDocId
            - Signee
        - Handler
            0. Convert CertifiedId => DocId
            1. Load DocId
            2. Update Sign State sesuai command
            3. Update DocState = "Signed"
            4. Write DocModel
        - Response [none]
        - Event Subscriber [none]
        - Note: Command ini di-trigger ketika ada callback signed document dari tekenAja!

    9. Publish
        - Command
            - CertifiedDocId
            - UrlDownloadDoc
        - Handler
            0. Execute DownloadTekenAjaWorker
            1. Convert CertifiedDocId => DocId
            2. Load DocId
            3. Update DocUrl
            4. Update DocState = "Published"
            5. Writer DocModel
        - Resnponse [none]
        - Event Subscriber
            - NotifDocOwnerWorker
        - Note: Command ini di-trigger ketika ada callback Complete Signed Document dari tekenAja!
