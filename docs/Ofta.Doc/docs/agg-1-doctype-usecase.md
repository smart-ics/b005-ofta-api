 # USE-CASE DOCTYPE MODEL

1. DocTypeModel
    1. Create
        - Command
            - DocTypeName
        - Handler
            1. Create DocTypeModel
            2. Set DocTypeName
            3. Write DocTypeModel
        - Response
            - DocTypeId
        - Event Subscriber [none]

    2. Template
        - Command
            - DocTypeId
            - TemplateUrl
        - Handler
            1. Load DocTypeId
            2. Set TemplateUrl
            3. Set TemplateType
            4. Set Flag IsTemplate
            5. Write DocTypeModel
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

