import pandas as pd
import plotly.graph_objects as go
from plotly.subplots import make_subplots
import math

class TreeMapGraph:
    """
    TreeMapGraph is responsible for creating the hierarchical treemap of movies and genres.

    Parameters
    ----------
    data : Dataframe
        The dataframe that holds the information from the CSV.

    Methods
    -------
    getAllGenres()
        Gets all genres present in the data
    createAllGenres()
        Creates a list of all present genres
    getFigure()
        Gets the plotly figure
    createMovieTreeMap()
        Creates the hierarchical treemap.
    """
    def __init__(self, data):
        """
        Parameters
        ----------
        :param data: Dataframe
            The dataframe that holds the information from the CSV.
        """
        self.data = data
        self.allGenres = []
        self.platforms = ['Netflix', 'Hulu', 'Prime Video', 'Disney+']
        self.platColors = ['#E50914', '#1CE783', '#00A8E1', '#1038CD']
        self.markerColors = []
        self.createAllGenres()
        self.datafAllTrees = self.createMovieTreemap()
        for color in self.platColors:
            self.markerColors.append(color)
        self.markerColors.append("#002669")
        self.figure = go.Figure(go.Treemap(
            labels=self.datafAllTrees['label'],
            ids=self.datafAllTrees['id'],
            parents=self.datafAllTrees['parent'],
            values=self.datafAllTrees['value'],
            branchvalues='remainder',
            marker_colors=self.markerColors,
            hovertemplate='<b>%{label} </b> <br> Number Of Movies: %{value}<extra></extra>',
            maxdepth=2,
        ),
            layout={
                'paper_bgcolor': "#272D3F",
                'font': {
                    'color': '#ffffff'
                }
            })
    pass

    def getAllGenres(self):
        """
        Returns
        -------
        :return: list
            Returns a list of all generes
        """
        return self.allGenres

    def createAllGenres(self) -> None:
        """
        Returns
        -------
        :return: None
        """
        genres = []
        for genreStr in self.data["Genres"].tolist():
            if isinstance(genreStr, str):
                genreList = genreStr.split(",")
                for genre in genreList:
                    genres.append(genre.strip())
        self.allGenres = list(dict.fromkeys(genres))
        print()
    pass

    def getFigure(self):
        """
        Returns
        -------
        :return: plotly figure
            The treemap figure that was generated.
        """
        return self.figure
    pass

    def createMovieTreemap(self) -> pd.DataFrame:
        """
        Returns
        -------
        :return: DataFrame
            The data frame that will hold the hierarchical information of the tree maps.
        """
        allDfgGenres = {}
        for i, gen in enumerate(self.allGenres):
            self.data['Genres'] = self.data['Genres'].apply(str)
            dfg = self.data[self.data['Genres'].str.contains(gen, case=False)]
            allDfgGenres[gen] = dfg
        datafAllTrees = pd.DataFrame(columns=['label', 'id', 'parent', 'value', 'color'])
        totalData = len(self.data)
        sumofGen = 0
        for i, platform in enumerate(self.platforms):
            if i > len(self.platforms):
                continue
            for gen in self.allGenres:
                self.markerColors.append("#EEEEEE")
                numberOfGen = len(allDfgGenres[gen][allDfgGenres[gen][platform] == 1])
                sumofGen += len(allDfgGenres[gen][allDfgGenres[gen][platform] == 1])
                totalNumOnPlat = len(self.data[self.data[platform] == 1])
                seriesRow = pd.Series(dict(label=gen, id=gen + platform, parent=platform, value=numberOfGen,
                                           color=numberOfGen / totalNumOnPlat))
                datafAllTrees = datafAllTrees.append(seriesRow, ignore_index=True)
        # add platform
        for platform in self.platforms:
            numberOfPlat = len(self.data[self.data[platform] == 1])
            seriesRow = pd.Series(dict(label=platform, id=platform, parent='All Movies', value=numberOfPlat,
                                       color=numberOfPlat / totalData))
            datafAllTrees = datafAllTrees.append(seriesRow, ignore_index=True)
        total = pd.Series(dict(label='All Movies', id='All Movies', parent='', value=len(self.data), color=1))
        datafAllTrees = datafAllTrees.append(total, ignore_index=True)
        return datafAllTrees
    pass