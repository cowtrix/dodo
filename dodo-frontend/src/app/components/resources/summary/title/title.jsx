import React from "react"
import PropTypes from "prop-types"
import styles from "./title.module.scss"


export const Title = ({ title, type, parent, startDate, endDate }) => {
	const TypeName = (t) => {
		return t
			.replace(/^[a-z]|[A-Z]/g,
				function (v, i) {
					return (i === 0 || v.toUpperCase() === v) ? " " + v.toUpperCase() : v;
				});
	}
	const FormatDate = (d) => {
		d = new Date(d);
		if (!d) {
			return "";
		}
		var str = d.getDate() + '/' + d.getMonth();
		var delta = Date.now() - d;
		if (delta < 2 * 24 * 60 * 60 * 1000) {

			var hours = d.getHours();
			if (hours < 10) {
				hours = '0' + hours;
			}
			var minutes = d.getMinutes();
			if (minutes < 10) {
				minutes = '0' + minutes;
			}
			str += ' ' + hours + ':' + minutes;
		}
		return str;
	}
	const FormatDateRange = (d1, d2) => {
		return FormatDate(d1) + ' - ' + FormatDate(d2);
	}
	return (
		<div className={styles.titleContainer}>
			<h2 className={styles.title}>{title}</h2>
			<h4 className={styles.location}>{TypeName(type)}</h4>
			{parent ? <h4 className={styles.location}>{parent.name}</h4> : null}
			{startDate || endDate ? <h5 className={styles.location}>{FormatDateRange(startDate, endDate)}</h5> : null}
		</div>
	)
}

Title.propTypes = {
	title: PropTypes.string,
	parent: PropTypes.oneOfType([
		PropTypes.string,
		PropTypes.object
	]),
	type: PropTypes.string,
	startDate: PropTypes.instanceOf(Date),
	endDate: PropTypes.instanceOf(Date),
}
