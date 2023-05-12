# Electricity-Data

### Purpose
Create an application that retrieves public electricity data and stores aggregated data into a database.
Expose API GET endpoint where could retrieve aggregated data.

### Requirements
- Use https://data.gov.lt/dataset/siame-duomenu-rinkinyje-pateikiami-atsitiktinai-parinktu-1000-buitiniu-vartotoju-automatizuotos-apskaitos-elektriniu-valandiniai-duomenys datasets
- Use HTTP Client
- Datasets can be downloaded via:
  - https://data.gov.lt/dataset/1975/download/10766/2022-05.csv
  - https://data.gov.lt/dataset/1975/download/10765/2022-04.csv
  - https://data.gov.lt/dataset/1975/download/10764/2022-03.csv
  - https://data.gov.lt/dataset/1975/download/10763/2022-02.csv
- Process last four months' data
- Filter only apartament (Butas) data
- Store data into a database grouped by Tinklas (Regionas) field and apply aggregation:
  - Sum P+ fields
  - Sum P- fields
- For database, communication use Dapper or EF Core
- Add logging
- Write unit tests for the main flow
- The app must run on docker
