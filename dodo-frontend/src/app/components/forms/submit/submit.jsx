import React from "react";
import PropTypes from "prop-types";
import { Button } from "../../button";
import styles from "./submit.module.scss";

export const Submit = ({ submit, value, className }) => (
	<div className={styles.submit}>
		<Button variant="cta" onClick={() => submit()} className={className}>
			<div>{value}</div>
		</Button>
	</div>
);

Submit.propTypes = {
	submit: PropTypes.func,
	value: PropTypes.string,
	className: PropTypes.string,
};
