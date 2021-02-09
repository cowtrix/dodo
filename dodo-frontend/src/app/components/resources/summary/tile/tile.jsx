import React from "react";
import PropTypes from "prop-types";

import styles from "./tile.module.scss";

export const Tile = ({ type, resourceTypes = [] }) => {
	const eventColor = resourceTypes.find(
		(resType) => type.toLowerCase() === resType.value.toLowerCase()
	)?.displayColor || "808080";

	const tileStyles = {
		color: "black",
		backgroundColor: "#" + eventColor,
	};

	return <div className={styles.tile} style={tileStyles}></div>;
};

Tile.propTypes = {
	event: PropTypes.object,
	resourceTypes: PropTypes.array,
};
