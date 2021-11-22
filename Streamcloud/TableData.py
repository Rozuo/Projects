import dash_table

class TableData:
    """
    The table data is responsible for creating a table of movies or TV shows.

    Parameters
    ----------
    data : Dataframe
        The dataframe that holds the information from the CSV.
    movies : bool
        Determines whether the data is for movies or TV shows.
    search : bool
        Determines whether the table should be made for searching.

    Methods
    -------
    getDataTable()
        Returns the data table
    getCleanData()
        Returns the data frame that has been pre-processed
    cleanData()
        Drops unnecessary columns for the user.
    groupPlatforms()
        Bins all the platforms of the data.
    """
    def __init__(self, data, movies=True, search=True):
        """
        Parameters
        ----------
        :param data: Dataframe
            Holds the data from a CSV
        :param movies: bool
            Determines whether the data is for movies or TV shows.
        :param search: bool
            Determines whether the table should be made for searching.
        """
        self.data = data
        self.headerName = data.columns.values
        self.cleanData()
        self.groupPlatforms()
        if movies and not search:
            self.dataTable = dash_table.DataTable(
                id="Data-Table",
                sort_action="native",
                columns=[{"name": i, "id": i} for i in self.data.columns],
                data=self.data.to_dict('records'),
                style_as_list_view=True,
                style_header={
                    'backgroundColor': '#3B4154',
                    'overflow': 'hidden'
                },
                style_data={
                    'padding-right': '2rem',
                },
                style_table={
                    'overflowY': 'auto',
                    'maxWidth': '100%',
                    'height': '40rem'
                },
                style_cell_conditional=[
                    {'if': {'column_id': 'Title'},
                    'width': '28%'},
                    {'if': {'column_id': 'Year'},
                    'width': '4%'},
                    {'if': {'column_id': 'Age'},
                    'width': '4%'},
                    {'if': {'column_id': 'IMDb'},
                    'width': '4%'},
                    {'if': {'column_id': 'RT'},
                    'width': '4%'},
                    {'if': {'column_id': 'Country'},
                    'width': '10%'},
                    {'if': {'column_id': 'Genres'},
                    'width': '10%'},
                    {'if': {'column_id': 'Directors'},
                    'width': '10%'},
                    {'if': {'column_id': 'Language'},
                    'width': '10%'},
                    {'if': {'column_id': 'Runtime'},
                    'width': '6%'},
                    {'if': {'column_id': 'Platform'},
                    'width': '10%'},
                ],
                style_header_conditional=[
                    {'if': {'column_id': 'Platform'},
                    'padding-left': '1rem'},
                    {'if': {'column_id': 'Title'},
                    'padding-left': '1rem'}
                ],
                style_cell={
                    'height': 'auto',
                    'whiteSpace': 'normal',
                    'width-max': '120px',
                    'textAlign': 'left',
                    'color': '#FFFFFF',
                    'border': 'none',
                    'font-family': 'Roboto',
                    'font-weight': 'normal',
                    'maxWidth': 95
                },
                style_data_conditional=[
                    {
                    'if': {'row_index': 'even'},
                        'backgroundColor': '#272d3f'},
                    {'if': {'row_index': 'odd'},
                        'backgroundColor': '#2c3245'},
                    {'if': {'column_id': 'Platform'},
                        'padding-left': '1rem'},
                    {'if': {'column_id': 'Title'},
                        'padding-left': '1rem'}
                ],
                fixed_rows={'headers': True, 'data': 0}
            )
            
        if not movies and not search:
            self.dataTable = dash_table.DataTable(
                id="Data-Table-TV",
                sort_action="native",
                columns=[{"name": i, "id": i} for i in self.data.columns],
                data=self.data.to_dict('records'),
                style_as_list_view=True,
                style_header={
                    'backgroundColor': '#3B4154',
                    'overflow': 'hidden'
                },
                style_data={
                    'padding-right': '2rem',
                },
                style_table={
                    'overflowY': 'auto',
                    'maxWidth': '100%',
                    'height': '40rem'
                },
                style_cell_conditional=[
                    {'if': {'column_id': 'Title'},
                    'width': '30%'},
                ],
                style_header_conditional=[
                    {'if': {'column_id': 'Title'},
                    'padding-left': '1rem'}
                ],
                style_cell={
                    'height': 'auto',
                    'whiteSpace': 'normal',
                    'width-max': '120px',
                    'textAlign': 'left',
                    'color': '#FFFFFF',
                    'border': 'none',
                    'font-family': 'Roboto',
                    'font-weight': 'normal',
                    'maxWidth': 95
                },
                style_data_conditional=[
                    {
                    'if': {'row_index': 'even'},
                        'backgroundColor': '#272d3f'
                    },
                    {
                    'if': {'row_index': 'odd'},
                        'backgroundColor': '#2c3245'
                    },
                    {'if': {'column_id': 'Title'},
                    'padding-left': '1rem'}
                ],
                fixed_rows={'headers': True, 'data': 0}
            )

        if movies and search:
            self.dataTable = dash_table.DataTable(
                id="Data-Table-Search",
                sort_action="native",
                columns=[{"name": i, "id": i} for i in self.data.columns],
                data=self.data.to_dict('records'),
                style_as_list_view=True,
                style_header={
                    'backgroundColor': '#3B4154',
                    'overflow': 'hidden'
                },
                style_data={
                    'padding-right': '2rem',
                },
                style_table={
                    'overflowY': 'auto',
                    'maxWidth': '100%',
                    'height': '80rem'
                },
                style_cell_conditional=[
                    {'if': {'column_id': 'Title'},
                    'width': '28%'},
                    {'if': {'column_id': 'Year'},
                    'width': '4%'},
                    {'if': {'column_id': 'Age'},
                    'width': '4%'},
                    {'if': {'column_id': 'IMDb'},
                    'width': '4%'},
                    {'if': {'column_id': 'RT'},
                    'width': '4%'},
                    {'if': {'column_id': 'Country'},
                    'width': '10%'},
                    {'if': {'column_id': 'Genres'},
                    'width': '10%'},
                    {'if': {'column_id': 'Directors'},
                    'width': '10%'},
                    {'if': {'column_id': 'Language'},
                    'width': '10%'},
                    {'if': {'column_id': 'Runtime'},
                    'width': '6%'},
                    {'if': {'column_id': 'Platform'},
                    'width': '10%'},
                ],
                style_header_conditional=[
                    {'if': {'column_id': 'Platform'},
                    'padding-left': '1rem'},
                    {'if': {'column_id': 'Title'},
                    'padding-left': '1rem'}
                ],
                style_cell={
                    'height': 'auto',
                    'whiteSpace': 'normal',
                    'width-max': '120px',
                    'textAlign': 'left',
                    'color': '#FFFFFF',
                    'border': 'none',
                    'font-family': 'Roboto',
                    'font-weight': 'normal',
                    'maxWidth': 95
                },
                style_data_conditional=[
                    {
                    'if': {'row_index': 'even'},
                        'backgroundColor': '#272d3f'},
                    {'if': {'row_index': 'odd'},
                        'backgroundColor': '#2c3245'},
                    {'if': {'column_id': 'Platform'},
                        'padding-left': '1rem'},
                    {'if': {'column_id': 'Title'},
                        'padding-left': '1rem'}
                ],
                fixed_rows={'headers': True, 'data': 0}
            )

        if not movies and search:
            self.dataTable = dash_table.DataTable(
                id="Data-Table-TV-Search",
                sort_action="native",
                columns=[{"name": i, "id": i} for i in self.data.columns],
                data=self.data.to_dict('records'),
                style_as_list_view=True,
                style_header={
                    'backgroundColor': '#3B4154',
                    'overflow': 'hidden'
                },
                style_data={
                    'padding-right': '2rem',
                },
                style_table={
                    'overflowY': 'auto',
                    'maxWidth': '100%',
                    'height': '80rem'
                },
                style_cell_conditional=[
                    {'if': {'column_id': 'Title'},
                    'width': '30%'},
                ],
                style_header_conditional=[
                    {'if': {'column_id': 'Title'},
                    'padding-left': '1rem'}
                ],
                style_cell={
                    'height': 'auto',
                    'whiteSpace': 'normal',
                    'width-max': '120px',
                    'textAlign': 'left',
                    'color': '#FFFFFF',
                    'border': 'none',
                    'font-family': 'Roboto',
                    'font-weight': 'normal',
                    'maxWidth': 95
                },
                style_data_conditional=[
                    {
                    'if': {'row_index': 'even'},
                        'backgroundColor': '#272d3f'
                    },
                    {
                    'if': {'row_index': 'odd'},
                        'backgroundColor': '#2c3245'
                    },
                    {'if': {'column_id': 'Title'},
                    'padding-left': '1rem'}
                ],
                fixed_rows={'headers': True, 'data': 0}
            )

    def getDataTable(self):
        """
        Returns
        -------
        :return: data table
            returns the created dash data table.
        """
        return self.dataTable

    def getCleanData(self):
        """
        Returns
        -------
        :return: data frame
            Returns the pre-processed data frame.
        """
        return self.data

    def cleanData(self):
        """
        Returns
        -------
        :return: drops unnecessary columns for the user.
        """
        self.data = self.data.drop(['ID', 'Type'], axis=1)

    def groupPlatforms(self):
        """
        Returns
        -------
        :return: None
        """
        self.data.loc[self.data["Netflix"] == 1, 'Netflix'] = 'Netflix, '
        self.data.loc[self.data['Netflix'] == 0, 'Netflix'] = ''

        self.data.loc[self.data['Prime Video'] == 1, 'Prime Video'] = 'Prime Video, '
        self.data.loc[self.data['Prime Video'] == 0, 'Prime Video'] = ''

        self.data.loc[self.data['Hulu'] == 1, 'Hulu'] = 'Hulu, '
        self.data.loc[self.data['Hulu'] == 0, 'Hulu'] = ''

        self.data.loc[self.data['Disney+'] == 1, 'Disney+'] = 'Disney+, '
        self.data.loc[self.data['Disney+'] == 0, 'Disney+'] = ''

        self.data["Platform"] = self.data["Netflix"] + self.data["Prime Video"] + self.data["Hulu"] + \
                                self.data["Disney+"]
        self.data["Platform"] = self.data["Platform"].str.strip()
        self.data["Platform"] = self.data["Platform"].str.rstrip(",")
        self.data = self.data.drop(['Netflix', 'Prime Video', 'Hulu', 'Disney+'], axis=1)