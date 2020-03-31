import React from "react";
import PropTypes from "prop-types";
import Select from "react-select";

import styles from "./search-bar.module.scss";

export const SearchBar = ({ searchValues }) => (
	<Select className={styles.searchBar} placeholder="Search for sites in..." />
);

SearchBar.propTypes = {
	searchValues: PropTypes.array
};
