from tapnet import TapNet


class GameServer:
    def __init__(self):
        self.high_scores = []
        self.server = None

    def get_ordered_high_scores(self):
        """
        Organiza el diccionario de high scores en orden descendiente
        :return: Diccionario de los 10 high scores mas altos ordenado con formato 'player_name' : highscore
        """
        self.high_scores = sorted(self.high_scores, key=lambda player:player[1])
        return self.high_scores[:10]

    def get_player_rank(self, player_name, player_score):
        """
        :return: indice del jugador en la lista de high scores
        """
        return self.high_scores.index([d for d in self.high_scores if d[1] == player_score and d[0] == player_name][0])

    def handle_json(self, received_json, sender):
        """
        Reacciona en funcion al JSON recibido
        :param received_json: JSON que hemos recibido
        :param sender: Quien nos envia los datos
        """
        self.high_scores.append((received_json['name'], int(received_json['score'])))

        data_to_send = {
            'high_scores': self.get_ordered_high_scores(),
            'player_rank': self.get_player_rank(received_json['name'], int(received_json['score'])),
            'player_stats': [received_json['name'], int(received_json['score'])]
        }
        print(sender)
        self.server.send_json(data_to_send, TapNet.DATAGRAM_RELIABLE, sender)

    def start(self):
        """
        Arranca el servidor
        """
        server_address = ('127.0.0.1', 10000)
        self.server = TapNet(server_address)
        self.server.response_handler = self.handle_json
        self.server.start()
