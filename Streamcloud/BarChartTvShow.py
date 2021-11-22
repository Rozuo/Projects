import pandas as pd
import plotly.graph_objects as go


class BarChartTvShow:
    """
    This class is responsible for creating the bar charts for tv shows and hold information about the bar charts.

    Attributes
    ----------
    data : Dataframe
        The dataframe that holds the information from the CSV.

    Methods
    -------
    getFigure()
        Gets the bar chart figure.
    getPlatColors()
        Gets the colors associated with the bar charts.
    createBarChart()
        Creates the bar chart.
    """
    def __init__(self, data):
        """
        Parameters
        ----------
        :param data: dataframe
            The dataframe that holds the information from the CSV.
        """
        self.data = data
        self.platforms = ['Netflix', 'Prime Video', 'Hulu', 'Disney+']
        self.platColors = ['#E50914', '#00A8E1', '#1CE783', '#1038CD']

        self.figure = self.createBarChart()

    def getFigure(self):
        """
        Returns
        -------
        :return: plotly figure
            The figure created.
        """
        return self.figure

    def getPlatColors(self):
        """
        Returns
        -------
        :return: list
            Returns a list of colors associated with a platform.
        """
        return self.platColors

    def createBarChart(self):
        """
        Returns
        -------
        :return: plotly figure
            The figure that is created from the data.
        """
        countPlatforms = []
        for platform in self.platforms:
            countPlatforms.append(self.data[platform].sum())

        fig = go.Figure(data=[
            go.Bar(x=self.platforms, y=countPlatforms, marker_color=self.platColors,
                   hovertemplate='<b>%{label} </b> <br> Number Of TV Shows: %{value}<extra></extra>'
                   )],
            layout={
                'paper_bgcolor': "#272D3F",
                'plot_bgcolor': "#272D3F",
                'font': {
                    'color': '#ffffff'
                }
            })
        return fig
