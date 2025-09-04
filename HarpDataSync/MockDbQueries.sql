CREATE TABLE APP_Application (
    ApplicationID INT PRIMARY KEY,
    RefIRASProjectID NVARCHAR(50),
    CommitteeID INT,
    ApplicationTitle NVARCHAR(255),
    ApplicationDecisionText NVARCHAR(255),
    DateRegistered DATETIME,
    ParentApplicationID INT,
    PostApprovalStateText NVARCHAR(255),
    StudyTypeID INT
);

CREATE TABLE APP_Committee (
    CommitteeID INT PRIMARY KEY,
    Name NVARCHAR(255)
);

CREATE TABLE APP_StudyType (
    StudyTypeID INT PRIMARY KEY,
    StudyType NVARCHAR(255)
);

CREATE TABLE APP_ApplicationToProject (
    ApplicationID INT,
    ProjectID INT,
    IsInUse BIT,
    PRIMARY KEY (ApplicationID, ProjectID)
);

CREATE TABLE CR_ProjectData_LongText_1 (
    ProjectID INT PRIMARY KEY,
    field3 NVARCHAR(MAX)  -- Full Research Title
);


CREATE TABLE CR_ProjectData_Checkbox_2 (
    ProjectID INT PRIMARY KEY,
    SomeCheckboxValue BIT
);

CREATE TABLE CR_Project (
    ProjectID INT PRIMARY KEY,
    DataSetID INT
);

CREATE TABLE CR_FieldToDataset (
    DatasetID INT,
    FieldID INT,
    PRIMARY KEY (DatasetID, FieldID)
);

CREATE TABLE CR_DataField (
    FieldID INT PRIMARY KEY,
    FieldName NVARCHAR(255)
);

CREATE TABLE APP_ApplicationDetailsField (
    FieldID INT PRIMARY KEY,
    FieldValue NVARCHAR(255)
);

-- Mock daa

-- APP_Application
INSERT INTO APP_Application VALUES
(1, '1234567', 316, 'Study on AI', 'Favourable Opinion', '2023-01-15', NULL, 'Started', 6),
(2, '55555', 313, 'Study on ML', 'Further Information Favourable Opinion', '2023-02-20', NULL, 'Not Started', 6);

-- APP_Committee
INSERT INTO APP_Committee VALUES
(316, 'Committee A'),
(313, 'Committee B');

-- APP_StudyType
INSERT INTO APP_StudyType VALUES
(6, 'Clinical Trial'),
(7, 'Research Tissue Bank');

-- APP_ApplicationToProject
INSERT INTO APP_ApplicationToProject VALUES
(1, 1001, 1),
(2, 1002, 1);

-- CR_ProjectData_LongText_1
INSERT INTO CR_ProjectData_LongText_1 VALUES
(1001, 'Full Research Title for Project 1001'),
(1002, 'Full Research Title for Project 1002');

-- CR_ProjectData_Checkbox_2
INSERT INTO CR_ProjectData_Checkbox_2 VALUES
(1001, 1),
(1002, 0);

-- CR_Project
INSERT INTO CR_Project VALUES
(1001, 501),
(1002, 502);

-- CR_FieldToDataset
INSERT INTO CR_FieldToDataset VALUES
(501, 83),
(502, 83);

-- CR_DataField
INSERT INTO CR_DataField VALUES
(83, 'Confidentiality Advisory Group Submission');

-- APP_ApplicationDetailsField
INSERT INTO APP_ApplicationDetailsField VALUES
(83, 'Submitted to CAG');
