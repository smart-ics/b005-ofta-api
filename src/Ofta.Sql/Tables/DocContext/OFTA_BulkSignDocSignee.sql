CREATE TABLE OFTA_BulkSignDocSignee(

    Email VARCHAR(50) NOT NULL CONSTRAINT OFTA_BulkSignDocSignee_Email DEFAULT(''),
    SignTag VARCHAR(50) NOT NULL CONSTRAINT OFTA_BulkSignDocSignee_SignTag DEFAULT(''),
    SignPosition INT NOT NULL CONSTRAINT OFTA_BulkSignDocSignee_SignPosition DEFAULT(0),
    SignPositionDesc VARCHAR(200) NOT NULL CONSTRAINT OFTA_BulkSignDocSignee_SignPositionDesc DEFAULT(''), 
    SignUrl VARCHAR(200) NOT NULL CONSTRAINT OFTA_BulkSignDocSignee_SignUrl DEFAULT(''),   
)
GO

CREATE CLUSTERED INDEX CX_OFTA_BulkSignDocSignee ON OFTA_BulkSignDocSignee(DocId)
GO