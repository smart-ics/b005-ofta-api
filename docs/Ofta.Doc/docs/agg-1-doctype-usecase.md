 # USE-CASE DOCTYPE MODEL

1. DocTypeModel
    1. CreateDoc
        - Command
            - DocTypeName
        - Handler
            0. Create DocTypeModel
            1. Set DocTypeName
            2. Write DocTypeModel
        - Response
            - DocTypeId
        - Event Subscriber [none]

    2. Template
        - Command
            - DocTypeId
            - TemplateUrl
        - Handler
            0. Load DocTypeId
            1. Set TemplateUrl
            2. Set TemplateType
            3. Set Flag IsTemplate
            4. Write DocTypeModel
        - Response [none]
        - Event Subscribers [none]

    3. Activate
        - Command
            - DocTypeId
        - Handler
            0. Load DocTypeId
            1. Set IsAktif = true
            2. Write DocTypeModel
        - Response [none]
        - Event Subscribers [none]

    4. Deactivate
        - Command
            - DocTypeId
        - Handler
            0. Load DocTypeId
            1. Set IsAktif = false
            2. Write DocTypeModel
        - Response [none]
        - Event Subscribers [none]

    5. AddTag
        - Command
            - DocTypeId
            - Tag
        - Handler
            0. Load DocTypeId
            1. Add Tag sesuai Command
            2. Write DocTypeModel
        - Response [none]
        - Event Subscribers [none]

    6. RemoveTag
        - Command
            - DocTypeId
            - Tag
        - Handler
            0. Load DocTypeId
            1. remove Tag sesuai Command
            2. Write DocTypeModel
        - Response [none]
        - Event Subscribers [none]

